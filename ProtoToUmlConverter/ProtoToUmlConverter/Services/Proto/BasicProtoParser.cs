using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ProtoToUmlConverter.Services.Proto;

internal class BasicProtoParser : IProtoParser
{
    public IImmutableList<RawProtoBuffType> Parse(ProtoFile protoFile)
    {
        try
        {
            var nameSpace = GetNamespace(protoFile.Lines);
            var linesWithMessageNumber = GroupLinesByMessageNumber(protoFile);
            var rawProtoBuffTypes = linesWithMessageNumber
                .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key)) // We ignore group 0
                .SelectMany(kvp => GetRawProtoBuffTypeFromLines(kvp.Value, nameSpace))
                .ToImmutableList();
            return rawProtoBuffTypes;
        }
        catch (Exception)
        {
            Console.WriteLine($"Error while parsing {protoFile.FileName}");
            throw;
        }
    }

    private static IImmutableDictionary<string, IImmutableList<string>> GroupLinesByMessageNumber(ProtoFile protoFile)
    {
        var messageNames = new Stack<string>();
        messageNames.Push(string.Empty);
        GroupType? currentGroupType = null;
        ;
        var linesWithMessageNumber = new Dictionary<string, List<string>>();
        linesWithMessageNumber.Add(messageNames.Peek(), new List<string>());
        foreach (var line in protoFile.Lines)
        {
            if (IsAMessageDefinitionLine(line))
            {
                var currentMessageName = ExtractMessageNameFromLine(line);
                messageNames.Push(currentMessageName);
                currentGroupType = GroupType.Message;
                linesWithMessageNumber.Add(currentMessageName, new List<string>());
            }

            if (IsAnEnumDefinitionLine(line))
            {
                var enumName = ExtractEnumNameFromLine(line);
                messageNames.Push(enumName);
                currentGroupType = GroupType.Enum;
                linesWithMessageNumber.Add(enumName, new List<string>());
            }

            if (IsAOneOfDefinitionLine(line))
            {
                currentGroupType = GroupType.OneOf;
            }

            if (messageNames.Count > 0)
            {
                linesWithMessageNumber[messageNames.Peek()].Add(line);
            }

            if (IsAnEndOfGroup(line) && currentGroupType != GroupType.OneOf && messageNames.Count > 0)
            {
                messageNames.Pop();
            }
        }

        return linesWithMessageNumber.ToDictionary(
                k => k.Key,
                kvp => kvp.Value.ToImmutableList() as IImmutableList<string>)
            .ToImmutableDictionary();
    }

    private IImmutableList<RawProtoBuffType> GetRawProtoBuffTypeFromLines(
        IImmutableList<string> lines,
        string currentNamespace)
    {
        if (lines.Any(IsAMessageDefinitionLine))
        {
            var typeName = GetMessageName(lines);
            if (typeName == null)
            {
                return [];
            }

            var (dependencies, subTypes) = GetDependencies(lines, currentNamespace, typeName);

            var types = new List<RawProtoBuffType> { new(typeName!, currentNamespace, Kind.Message, dependencies, []) };
            types.AddRange(subTypes);
            return types.ToImmutableList();
        }

        if (lines.Any(IsAnEnumDefinitionLine))
        {
            var enumName = GetEnumName(lines);

            var enumValues = lines
                .Where(line => line.Contains("="))
                .Select(line =>
                    line.Split("=", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).First())
                .ToImmutableList();

            var enumType = new RawProtoBuffType(enumName, currentNamespace, Kind.Enumeration, [], enumValues);

            return [enumType];
        }

        return [];
    }

    private (IImmutableSet<RawDependency>, IImmutableSet<RawProtoBuffType>) GetDependencies(
        IImmutableList<string> lines,
        string currentNamespace,
        string typeName)
    {
        var groupNames = new Dictionary<int, string>();
        var groupDependencies = new Dictionary<int, List<RawDependency>>();
        groupDependencies.Add(0, new List<RawDependency>());
        var currentGroup = 0;
        var maxGroups = 0;

        foreach (var line in lines)
        {
            if (line.Trim().Contains($"{ProtobufDefinitions.FieldModifierOneOf} "))
            {
                maxGroups++;
                currentGroup = maxGroups;
                var fieldName = $"OneOf_{line.Trim().Split(" ")[1].Trim()}";
                var subTypeName = $"{typeName}_{fieldName}";
                groupNames[currentGroup] = subTypeName;
                groupDependencies.Add(currentGroup, new List<RawDependency>());
                groupDependencies[0].Add(new RawDependency(currentNamespace, subTypeName, fieldName, false, false));
            }

            if (!string.IsNullOrWhiteSpace(line)
                && !ProtobufDefinitions.Keywords.Contains(GetFirstNonWhiteSpaceElement(line))
                && line.Contains('=')
                && IsNotAnEnumValue(line))
            {
                groupDependencies[currentGroup].Add(GetRawDependency(line, currentNamespace));
            }

            if (line.Trim().Contains("}"))
            {
                currentGroup = 0;
            }
        }

        var subTypes = new List<RawProtoBuffType>();
        if (groupDependencies.Count > 1)
        {
            for (int i = 1; i < groupDependencies.Count; i++)
            {
                subTypes.Add(new RawProtoBuffType(
                    groupNames[i],
                    currentNamespace,
                    Kind.Message,
                    groupDependencies[i].ToImmutableHashSet(),
                    []));
            }
        }

        return (groupDependencies[0].ToImmutableHashSet(), subTypes.ToImmutableHashSet());
    }

    private bool IsNotAnEnumValue(string line)
    {
        return line
            .Trim()
            .Split("=")
            .First()
            .Split(" ")
            .Count(el => !string.IsNullOrEmpty(el)) > 1;
    }

    private RawDependency GetRawDependency(string line, string currentNamespace)
    {
        var typeElement = GetFirstNonWhiteSpaceElement(line);
        var fieldName = GetFieldName(line, typeElement);
        var isRepeated = line.Contains($"{ProtobufDefinitions.FieldModifierRepeated} ");
        var isOptional = line.Contains($"{ProtobufDefinitions.FieldModifierOptional} ");

        if (typeElement.Contains("."))
        {
            var indexLastSeparator = typeElement.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase);
            return new RawDependency(typeElement.Substring(0, indexLastSeparator),
                typeElement.Substring(indexLastSeparator + 1),
                fieldName,
                isRepeated,
                isOptional);
        }
        else if (ProtobufDefinitions.BaseTypes.Contains(typeElement))
        {
            return new RawDependency(string.Empty, typeElement, fieldName, isRepeated, isOptional);
        }
        else
        {
            return new RawDependency(currentNamespace, typeElement, fieldName, isRepeated, isOptional);
        }
    }

    private string GetFieldName(string line, string typeElement)
    {
        try
        {
            var indexOfTypeElement = line.IndexOf(typeElement);


            var fieldName = line
                .Substring(indexOfTypeElement + typeElement.Length)
                .Split("=", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .First();

            return fieldName;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static string GetFirstNonWhiteSpaceElement(string line)
    {
        return line.Split(" ").First(IsAValidType).Trim();
    }

    private static bool IsAValidType(string el)
    {
        return !string.IsNullOrWhiteSpace(el) && !ProtobufDefinitions.FieldModifiers.Contains(el.Trim());
    }

    private string? GetMessageName(IImmutableList<string> lines)
    {
        var lineWithMessage = lines
            .SingleOrDefault(line => IsAMessageDefinitionLine(line));

        if (lineWithMessage == null)
        {
            return null;
        }

        return ExtractMessageNameFromLine(lineWithMessage!);
    }

    private string? GetEnumName(IImmutableList<string> lines)
    {
        var lineWithMessage = lines.SingleOrDefault(line => IsAnEnumDefinitionLine(line));

        if (lineWithMessage == null)
        {
            return null;
        }

        return ExtractEnumNameFromLine(lineWithMessage!);
    }

    private static string ExtractMessageNameFromLine(string line)
    {
        return line
            .Replace("message", string.Empty)
            .Split(" ")
            .First(x => !string.IsNullOrWhiteSpace(x))
            .Trim();
    }

    private static string ExtractEnumNameFromLine(string line)
    {
        return line
            .Replace("enum", string.Empty)
            .Split(" ")
            .First(x => !string.IsNullOrWhiteSpace(x))
            .Trim();
    }

    private static bool IsAMessageDefinitionLine(string line)
    {
        return line.Trim().StartsWith("message");
    }

    private static bool IsAnEnumDefinitionLine(string line)
    {
        return line.Trim().StartsWith("enum");
    }

    private static bool IsAOneOfDefinitionLine(string line)
    {
        return line.Trim().StartsWith("oneof");
    }

    private static bool IsAnEndOfGroup(string line)
    {
        return line.Trim().EndsWith("}");
    }

    private string GetNamespace(IImmutableList<string> lines)
    {
        return lines
            .Single(line => line.StartsWith("package"))
            .Replace("package", string.Empty)
            .Replace(";", string.Empty)
            .Trim();
    }

    enum GroupType
    {
        Message,
        OneOf,
        Enum,
    }
}
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace TestDataFactoryGenerator.TestData.Served;

internal record AllCollections(
    ImmutableArray<PositionNote> PositionNoteImmutableArray,
    PositionNote[] PositionNoteArray,

    List<PositionNote> PositionNoteList,
    ImmutableList<PositionNote> PositionNoteImmutableList,
    IImmutableList<PositionNote> PositionNoteIImmutableList,
    IReadOnlyList<PositionNote> PositionNoteIReadOnlyList,

    IImmutableSet<PositionNote> PositionNoteIImmutableSet,
    ImmutableHashSet<PositionNote> PositionNoteImmutableHAshSet,
    ImmutableSortedSet<PositionNote> PositionNoteImmutableSortedSet,
    IReadOnlySet<PositionNote> PositionNoteIReadOnlySet,
    FrozenSet<PositionNote> PositionNoteFrozenSet,

    IImmutableDictionary<Guid, PositionNote> PositionNoteIImmutableDictionary,
    ImmutableDictionary<Guid, PositionNote> PositionNoteImmutableDictionary,
    ImmutableSortedDictionary<Guid, PositionNote> PositionNoteImmutableSortedDictionary,
    FrozenDictionary<Guid, PositionNote> PositionNoteFrozenDictionary,
    Dictionary<Guid, PositionNote> PositionNoteDictionary,

    FrozenSet<PositionNote>? OptionalPositionNoteFrozenSet);
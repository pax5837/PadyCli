using System.Collections.Frozen;
using System.Collections.Immutable;

namespace TestDataFactoryGenerator.TestData.Served;

internal record AllCollections(
    IEnumerable<PositionNote> PositionNoteIEnumerable,

    ICollection<PositionNote> PositionNoteICollection,
    IReadOnlyCollection<PositionNote> PositionNoteIReadonOnlyCollection,

    ImmutableArray<PositionNote> PositionNoteImmutableArray,
    PositionNote[] PositionNoteArray,

    List<PositionNote> PositionNoteList,
    LinkedList<PositionNote> PositionNoteLinkedList,
    ImmutableList<PositionNote> PositionNoteImmutableList,
    IImmutableList<PositionNote> PositionNoteIImmutableList,
    IReadOnlyList<PositionNote> PositionNoteIReadOnlyList,

    ISet<PositionNote> PositionNoteSet,
    IImmutableSet<PositionNote> PositionNoteIImmutableSet,
    ImmutableHashSet<PositionNote> PositionNoteImmutableHAshSet,
    ImmutableSortedSet<PositionNote> PositionNoteImmutableSortedSet,
    IReadOnlySet<PositionNote> PositionNoteIReadOnlySet,
    FrozenSet<PositionNote> PositionNoteFrozenSet,
    SortedSet<PositionNote> PositionNoteSortedSet,

    IImmutableDictionary<Guid, PositionNote> PositionNoteIImmutableDictionary,
    ImmutableDictionary<Guid, PositionNote> PositionNoteImmutableDictionary,
    ImmutableSortedDictionary<Guid, PositionNote> PositionNoteImmutableSortedDictionary,
    FrozenDictionary<Guid, PositionNote> PositionNoteFrozenDictionary,
    Dictionary<Guid, PositionNote> PositionNoteDictionary,

    Queue<PositionNote> PositionNoteQueue,
    Stack<PositionNote> PositionNoteStack,

    FrozenSet<PositionNote>? OptionalPositionNoteFrozenSet,

    ListWrapper<PositionNote, Delivery> PositionNoteListWrapper);
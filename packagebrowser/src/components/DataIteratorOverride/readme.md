this works, but I don't like it. tried a lot of different ways to extend the components but wasn't successful without copying the whole thing.

If you want you can take a look at it.

things that need to be changed in extended components:

1. `search` in filterableData's&#x200B;_(VData's)_ props should be `Object` instead of `String`
2. filterableDataIterator&#x200B;_(VDataIterator)_ should use FilterableData&#x200B;_(VData)_ instead of VData.

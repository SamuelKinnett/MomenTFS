namespace MomenTFS.TIM.DataEntry
{
    public class IndexedColourDataEntry : ImageDataEntry
    {
        public byte CLUTIndex { get; private set; }

        public IndexedColourDataEntry(byte clutIndex) {
            CLUTIndex = clutIndex;
        }
    }
}

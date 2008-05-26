namespace Parrano.Parser
{
    public class PSDscComment : PSComment
    {
        public override string UnParse()
        {
            return "%" + base.UnParse();
        }
    }
}
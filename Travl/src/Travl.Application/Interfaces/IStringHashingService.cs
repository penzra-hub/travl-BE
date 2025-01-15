namespace Travl.Application.Interfaces
{
    public interface IStringHashingService
    {
        public string CreateDESStringHash(string input);
        public string DecodeDESStringHash(string input);
    }
}

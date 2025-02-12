namespace Travl.Application.Interfaces
{
    public interface IStringHashingService
    {
        public string CreateAESStringHash(string input);
        public string DecodeAESStringHash(string input);
    }
}

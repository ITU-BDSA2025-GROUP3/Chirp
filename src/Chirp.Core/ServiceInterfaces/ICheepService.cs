namespace Chirp.Core.ServiceInterfaces;

public interface ICheepService
{
    int CurrentPage { get; set; }
    public Task<List<CheepDTO>> GetCheeps();
    public Task<int> GetTotalCheeps();
}

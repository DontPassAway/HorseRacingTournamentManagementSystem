namespace HorseRacing.Shared.Helpers;

public static class PaginationHelper
{
    public static (int skip, int take) GetPagination(int pageNumber, int pageSize)
    {
        int validPage = pageNumber < 1 ? 1 : pageNumber;
        int validSize = pageSize < 1 ? 10 : pageSize > 100 ? 100 : pageSize;
        return ((validPage - 1) * validSize, validSize);
    }
}

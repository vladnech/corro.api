namespace Correspondence.Api.DataAccess
{
    public static class SQLStatements
    {
        public static class InvestorDocuments
        {
            public static string ContainerName = "investor-docs";
            public static string ListInvestorDocs = "SELECT * FROM c WHERE c.InvestorId = @InvestorId";
        }
    }
}

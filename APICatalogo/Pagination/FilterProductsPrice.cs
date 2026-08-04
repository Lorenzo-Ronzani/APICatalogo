namespace APICatalogo.Pagination
{
    public class FilterProductsPrice : QueryStringParameters
    {
        public decimal? Price { get; set; }

        public string? CriteriaPrice { get; set; }

    }
}

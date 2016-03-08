namespace BookProgress.Web.Helpers.Utils
{
    public class SearchType
    {
        public string Route { get; private set; }

        public static SearchType Customer = new SearchType("/Customers/Modal");

        public static SearchType Contact = new SearchType("/Contact/SearchModal");

        public static SearchType LegalEntity = new SearchType("/LegalEntity/SearchModal");

        public static SearchType Account = new SearchType("/Account/SearchModal");

        public static SearchType Country = new SearchType("/Country/SearchModal");

        public static SearchType ZipCode = new SearchType("/ZipCode/SearchModal");

        private SearchType(string route)
        {
            Route = route;
        }
    }
}
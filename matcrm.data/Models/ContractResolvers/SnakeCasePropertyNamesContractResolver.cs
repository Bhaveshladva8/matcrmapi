namespace matcrm.data.ContractResolvers {
    public class SnakeCasePropertyNamesContractResolver : DeliminatorSeparatedPropertyNamesContractResolver {
        public SnakeCasePropertyNamesContractResolver() : base('_') {
        }
    }
}
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Models.Dto.Mollie;
using matcrm.data.Models.MollieModel;
using matcrm.data.Models.MollieModel.List;
using matcrm.data.Models.MollieModel.Url;

namespace matcrm.service.Services.Mollie {
    public abstract class OverviewClientBase<T> where T : IResponseObject {
        private readonly IMapper _mapper;

        protected OverviewClientBase(IMapper mapper) {
            this._mapper = mapper;
        }

        protected OverviewModel<T> Map(ListResponse<T> list) {
            return this._mapper.Map<OverviewModel<T>>(list);
        }

        protected UrlObjectLink<ListResponse<T>> CreateUrlObject(string url) {
            return new UrlObjectLink<ListResponse<T>> {
                Href = url
            };
        }
    }
}
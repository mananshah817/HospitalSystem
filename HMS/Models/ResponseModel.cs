using HMS.DataAccess.Infrastructure;

namespace HMS.Models
{
    public class ResponseModel
    {
        public ServiceResponse Status { get; set; }

    }
    public class ResponseModel<TModel>
    {
        public ServiceResponse Status { get; set; }
        public ServiceResponse<TModel> StatusModel { get; set; }

        public TModel Result { get; set; }
    }
}
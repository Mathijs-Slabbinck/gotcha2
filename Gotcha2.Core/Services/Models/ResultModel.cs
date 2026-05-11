namespace Gotcha2.Core.Services.Models
{
    public class ResultModel<T> : BaseResultModel
    {
        public T? Data { get; set; }
    }
}

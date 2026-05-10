namespace Gotcha2.Core.Services.Models
{
    public class BaseResultModel
    {
        public bool IsSuccess => !Errors.Any();
        public IList<string> Errors { get; set; } = new List<string>();
    }
}

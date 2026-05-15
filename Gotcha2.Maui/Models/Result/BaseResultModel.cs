namespace Gotcha2.Maui.Models.Result
{
    public class BaseResultModel
    {
        public bool IsSuccess => !Errors.Any();
        public IList<string> Errors { get; set; } = new List<string>();
    }
}

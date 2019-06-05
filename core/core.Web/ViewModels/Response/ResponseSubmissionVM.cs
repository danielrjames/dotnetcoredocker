using System.ComponentModel.DataAnnotations;

namespace core.Web.ViewModels.Response
{
    public class ResponseSubmissionVM
    {
        [Required]
        public string Response { get; set; }

        public ResponseSubmissionVM()
        {

        }
    }
}

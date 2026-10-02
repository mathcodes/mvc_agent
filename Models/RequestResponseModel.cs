
using System.ComponentModel.DataAnnotations;

namespace AI_Agent_MVC_App.Models;

public class RequestResponseModel
{
	[Required(ErrorMessage = "Enter a prompt before sending.")]
	[StringLength(8000, ErrorMessage = "Prompts must be 8,000 characters or fewer.")]
	[Display(Name = "Your prompt")]
	public string Prompt { get; set; } = string.Empty;

	public string? Response { get; set; }

	public string? ErrorMessage { get; set; }
}

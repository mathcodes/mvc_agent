
using AI_Agent_MVC_App.Models;
using AI_Agent_MVC_App.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI_Agent_MVC_App.Controllers;

public class AIController : Controller
{
	private readonly AIService _aiService;
	private readonly ILogger<AIController> _logger;

	public AIController(AIService aiService, ILogger<AIController> logger)
	{
		_aiService = aiService;
		_logger = logger;
	}

	[HttpGet]
	public IActionResult Index()
	{
		return View(new RequestResponseModel());
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Index(RequestResponseModel model, CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		try
		{
			model.Response = await _aiService.GetResponseAsync(model.Prompt, cancellationToken);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "The AI request failed.");
			model.ErrorMessage = exception.Message;
		}

		return View(model);
	}

	[HttpGet]
	public IActionResult Error()
	{
		return View("Error", new ErrorViewModel
		{
			RequestId = HttpContext.TraceIdentifier
		});
	}
}

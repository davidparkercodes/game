using System.Threading.Tasks;
using Game.Application.Shared.Cqrs;
using Game.Application.Game.Commands;
using Game.Application.Game.Queries;
using Game.Application.Rounds.Commands;
using Game.Infrastructure.Managers;
using Game.Infrastructure.DI;

namespace Game.Application.Game;

public class GameApplicationService
{
    private readonly IMediator _mediator;

    public GameApplicationService(IMediator mediator)
    {
        _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
    }

    public static GameApplicationService Instance { get; private set; }

    public static void Initialize()
    {
        var mediator = ServiceLocator.Instance.Resolve<IMediator>();
        Instance = new GameApplicationService(mediator);
    }

    public async Task<SpendMoneyResult> SpendMoneyAsync(int amount, string reason = null)
    {
        var command = new SpendMoneyCommand(amount, reason);
        return await _mediator.SendAsync<SpendMoneyResult>(command);
    }

    public async Task<StartRoundResult> StartRoundAsync(int roundNumber = 0)
    {
        var command = new StartRoundCommand(roundNumber);
        return await _mediator.SendAsync<StartRoundResult>(command);
    }

    public async Task<StartRoundResult> ForceStartDefendPhaseAsync()
    {
        var command = new StartRoundCommand(forceStart: true);
        return await _mediator.SendAsync<StartRoundResult>(command);
    }

    public async Task<SpendMoneyResult> TrySpendMoney(int amount, string reason = null)
    {
        try
        {
            return await SpendMoneyAsync(amount, reason);
        }
        catch (System.Exception ex)
        {
            return SpendMoneyResult.Failed($"Error spending money: {ex.Message}", GetCurrentMoney());
        }
    }

    private int GetCurrentMoney()
    {
        return GameManager.Instance?.Money ?? 0;
    }
}

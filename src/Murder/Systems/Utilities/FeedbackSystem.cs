using Bang.Contexts;
using Bang.Systems;
using Murder.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Systems;

[Filter(ContextAccessorFilter.None)]
public class FeedbackSystem : IStartupSystem
{
    public void Start(Context context)
    {
        if (!string.IsNullOrWhiteSpace(Game.Profile.FeedbackUrl))
        {
            _ = FeedbackServices.SendFeedbackAsync(Game.Profile.FeedbackUrl, "The game have started!");
        }
    }
}

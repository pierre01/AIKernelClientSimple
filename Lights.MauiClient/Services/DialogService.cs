﻿using AIKernelClient.Services.Interfaces;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;


namespace AIKernelClient.Services;

public class DialogService : IDialogService
{
    public async Task<bool> ShowAlert(string title, string message, string accept, string cancel)
    {
        var page = Shell.Current.CurrentPage;
        bool answer = await page.DisplayAlert(title, message, accept, cancel);
        return answer;
    }

    public async Task ShowAlert(string title, string message,  string cancel)
    {
        var page = Shell.Current.CurrentPage;
        await page.DisplayAlert(title, message,cancel,FlowDirection.MatchParent);
    }

    public async Task ShowToast(string message)
    {
#if WINDOWS
        var page = Shell.Current.CurrentPage;
        await page.DisplayAlert("", message, "OK");
#else
        CancellationTokenSource cancellationTokenSource = new();
        ToastDuration duration = ToastDuration.Long;
        double fontSize = 18;
        var toast = Toast.Make(message, duration, fontSize);

        await toast.Show(cancellationTokenSource.Token);
#endif
    }

}

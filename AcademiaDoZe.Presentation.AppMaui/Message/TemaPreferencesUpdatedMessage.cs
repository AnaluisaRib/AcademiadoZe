// Ana Luisa Ribeiro de Araujo

using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AcademiaDoZe.Presentation.AppMaui.Message;

public sealed class TemaPreferencesUpdatedMessage(string value)
    : ValueChangedMessage<string>(value)
{
}
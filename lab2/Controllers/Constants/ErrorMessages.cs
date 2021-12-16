using Trs.Extensions;

namespace Trs.Controllers.Constants;

public static class ErrorMessages
{
    private const string ProjectNotFoundByCode = "Nie znaleziono projektu z kodem \"{0}\".";
    private const string ProjectAlreadyExisting = "Projekt z kodem \"{0}\" już istnieje.";
    private const string ProjectNoLongerActive = "Projekt z kodem \"{0}\" jest zamknięty.";

    private const string AcceptedTimeNotFoundByCode = "Nie znaleziono zaakceptowanego czasu dla projektu z kodem \"{0}\".";
    private const string AcceptedTimeAlreadyExisting = "Zaakceptowany czas dla projektu z kodem \"{0}\" już istnieje.";
    private const string NoAccessToAcceptedTimeOfUserInMonth = "Brak uprawnień do zarządzania zaakceptowanym czasem pracy użytkownika \"{0}\" dla miesiąca {1}.";
    public const string AcceptedTimeNegative = "Wartość zaakceptowanego czasu nie może być mniejsza od 0.";

    private const string ReportFrozenForMonth = "Raport na miesiąc {0} został już zatwierdzony.";

    private const string ReportEntryNotFoundByMonthAndId = "Nie znaleziono wpisu o indeksie {1} dla miesiąca {0}.";

    private const string UserNotFoundByUsername = "Nie znaleziono użytkownika o nazwie \"{0}\".";
    private const string UserAlreadyExisting = "Użytkownik o nazwie \"{0}\" już istnieje.";

    public const string HasToBeLoggedIn = "Wyświetlenie strony wymaga zalogowania.";
    public const string MustNotBeLoggedIn = "Strona nie może zostać wyświetlona przez zalogowanego użytkownika.";

    public static string GetProjectNotFoundMessage(string code) =>
        string.Format(ProjectNotFoundByCode, code);
    public static string GetProjectAlreadyExistingMessage(string code) =>
        string.Format(ProjectAlreadyExisting, code);
    public static string GetProjectNoLongerActiveMessage(string code) =>
        string.Format(ProjectNoLongerActive, code);

    public static string GetAcceptedTimeNotFoundMessage(string code) =>
        string.Format(AcceptedTimeNotFoundByCode, code);
    public static string GetAcceptedTimeAlreadyExistingMessage(string code) =>
        string.Format(AcceptedTimeAlreadyExisting, code);
    public static string GetNoAccessToAcceptedTimeMessage(string username, string month) =>
        string.Format(NoAccessToAcceptedTimeOfUserInMonth, username, month);

    public static string GetReportFrozenMessage(string month) =>
        string.Format(ReportFrozenForMonth, month);

    public static string GetReportEntryNotFoundMessage(DateTime month, int id) =>
        string.Format(ReportEntryNotFoundByMonthAndId, month.ToMonthString(), id);

    public static string GetUserNotFoundMessage(string username) =>
        string.Format(UserNotFoundByUsername, username);
    public static string GetUserAlreadyExistingMessage(string username) =>
        string.Format(UserAlreadyExisting, username);
}

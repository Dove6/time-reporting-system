import ReportEntryModel from "./ReportEntryModel";
import MonthlyReportModel from "./MonthlyReportModel";

type DailyReportModel = MonthlyReportModel & {
    entries: ReportEntryModel[];
};

export default DailyReportModel;

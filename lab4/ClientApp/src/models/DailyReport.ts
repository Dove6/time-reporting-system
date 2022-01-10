import ReportEntry from "./ReportEntry";
import MonthlyReport from "./MonthlyReport";

type DailyReport = MonthlyReport & {
    entries: ReportEntry[];
};

export default DailyReport;

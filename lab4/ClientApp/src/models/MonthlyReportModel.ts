import ProjectTimeSummary from "./ProjectTimeSummary";

type MonthlyReportModel = {
    frozen: boolean;
    projectTimeSummaries: ProjectTimeSummary[];
};

export default MonthlyReportModel;

import ProjectTimeSummary from "./ProjectTimeSummary";

type MonthlyReport = {
    frozen: boolean;
    projectTimeSummaries: ProjectTimeSummary[];
};

export default MonthlyReport;

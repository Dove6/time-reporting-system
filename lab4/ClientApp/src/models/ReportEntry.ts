import ReportEntryUpdateRequest from "./ReportEntryUpdateRequest";

type ReportEntry = ReportEntryUpdateRequest & {
    date: string;
    projectCode: string;
}

export default ReportEntry;

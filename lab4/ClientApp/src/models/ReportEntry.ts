import ReportEntryUpdateRequest from "./ReportEntryUpdateRequest";

type ReportEntry = ReportEntryUpdateRequest & {
    id: number;
    date: string;
    projectCode: string;
}

export default ReportEntry;

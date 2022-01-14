import ReportEntryUpdateRequest from "./ReportEntryUpdateRequest";

type ReportEntryModel = ReportEntryUpdateRequest & {
    date: string;
    projectCode: string;
}

export default ReportEntryModel;

import React, {useContext, useEffect, useState} from 'react';
import {LastDateContext} from "../App";
import DailyReportModel from "../models/DailyReportModel";
import { Table } from "react-bootstrap";
import ProjectModel from "../models/ProjectModel";
import ReportEntryCreationRequest from "../models/ReportEntryCreationRequest";
import DailySummary from "../reports/DailySummary";
import '../custom.css';
import ReportEntryUpdateRequest from "../models/ReportEntryUpdateRequest";
import ReportEntryModel from "../models/ReportEntryModel";
import ApiConnector from "../ApiConnector";
import ReportEntry from "./ReportEntry";
import EditableReportEntry from "./EditableReportEntry";
import ReportEntryCreator from "./ReportEntryCreator";

export default function DailyReport() {
    const lastDateState = useContext(LastDateContext);

    const [dailyReport, setDailyReport] = useState<DailyReportModel | null>(null);
    const refreshDailyReport = () => {
        ApiConnector.getDailyReport(lastDateState.state.lastDate)
            .then(data => setDailyReport(data));
    }
    useEffect(refreshDailyReport, [lastDateState.state.lastDate]);

    const [projectList, setProjectList] = useState<ProjectModel[] | null>(null);
    useEffect(() => {
        ApiConnector.getProjects()
            .then(data => setProjectList(data))
            .catch(error => alert(error));
    }, []);

    const [modifiedEntryId, setModifiedEntryId] = useState<number | null>(null);

    const updateEntry = (updateRequest: ReportEntryUpdateRequest) => {
        if (modifiedEntryId === null)
            return;
        ApiConnector.updateReportEntry(modifiedEntryId, updateRequest)
            .then(() => {
                setModifiedEntryId(null);
                refreshDailyReport();
            })
            .catch(error => alert(error));
    };

    const appendEntry = (addedEntry: ReportEntryCreationRequest) => {
        return ApiConnector.addReportEntry(lastDateState.state.lastDate, addedEntry)
            .then(() => {
                refreshDailyReport();
                return Promise.resolve();
            })
            .catch(error => alert(error));
    };

    const removeEntry = (id: number) => {
        ApiConnector.deleteReportEntry(id)
            .then(() => {
                refreshDailyReport();
            })
            .catch(error => alert(error));
    }

    const getEditView = (id: number, entry: ReportEntryModel) => (
        <EditableReportEntry key={id} original={entry}
            categories={projectList?.filter(e => e.code === entry.projectCode)[0]?.categories ?? [{ code: '' }]}
            onConfirmClick={modified => updateEntry(modified)} onCancelClick={() => setModifiedEntryId(null)} />
    );

    const getDisplayView = (id: number, entry: ReportEntryModel) => (
        <ReportEntry key={id} content={entry} disabled={dailyReport?.frozen ?? false}
             onModifyClick={() => setModifiedEntryId(id)} onRemoveClick={() => removeEntry(id)} />
    );

    const getReportEntriesDict = () =>
        Object.entries(dailyReport?.entries ??  {}).map(e => ({ key: Number(e[0]), value: e[1] }));

    return (
        <>
            <h1>Raport czasu pracy na dzień{' '}
                <input type="date"
                   value={lastDateState.state.lastDate}
                   onChange={evt => lastDateState.setLastDate(evt.target.value)}
                />
            </h1>
            {dailyReport?.frozen ? <p className="lead">[Raport został zatwierdzony]</p> : <></>}
            <Table striped bordered hover>
                <thead>
                    <tr>
                        <th className="shrinked">Projekt</th>
                        <th className="shrinked">Kategoria</th>
                        <th className="shrinked">Czas (w minutach)</th>
                        <th>Opis</th>
                        <th className="shrinked">Akcje</th>
                    </tr>
                </thead>
                <tbody>
                    {getReportEntriesDict().map(({key, value}) => key === modifiedEntryId ?
                        getEditView(key, value) :
                        getDisplayView(key, value))}
                </tbody>
                <tfoot style={{ verticalAlign: 'top' }}>
                    <tr>
                        <td colSpan={5} style={{ borderLeft: 'hidden', borderRight: 'hidden' }}> </td>
                    </tr>
                    <tr>
                        <th colSpan={5}>Dodaj wpis...</th>
                    </tr>
                    <ReportEntryCreator projects={projectList} onConfirmClick={created => appendEntry(created)} />
                </tfoot>
            </Table>
            {dailyReport !== null && projectList !== null ? (<>
                <h2>Podsumowanie dzienne</h2>
                <DailySummary entries={dailyReport.entries} projects={projectList} />
            </>) : <></>}
        </>
    );
}

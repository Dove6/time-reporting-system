import React, {useContext, useEffect, useState} from 'react';
import {LastDateContext} from "../App";
import DailyReportModel from "../models/DailyReportModel";
import {Button, ButtonToolbar, Form, Table} from "react-bootstrap";
import ProjectModel from "../models/ProjectModel";
import ReportEntryCreationRequest from "../models/ReportEntryCreationRequest";
import DailySummary from "../reports/DailySummary";
import '../custom.css';
import ReportEntryUpdateRequest from "../models/ReportEntryUpdateRequest";
import getSpecificSetter from "../getSpecificSetter";
import ReportEntryModel from "../models/ReportEntryModel";
import ApiConnector from "../ApiConnector";
import ReportEntry from "./ReportEntry";
import EditableReportEntry from "./EditableReportEntry";

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
            .then(data => {
                setProjectList(data);
                if (data.length > 0)
                    setAddedEntryProject(data[0].code);
            });
    }, []);

    const [addedEntry, setAddedEntry] = useState<ReportEntryCreationRequest>({
        projectCode: '',
        categoryCode: '',
        time: 1,
        description: ''
    });
    const setAddedEntryProject = (projectCode: string) => setAddedEntry(prevState => ({...prevState, projectCode: projectCode, categoryCode: '' }));
    const setAddedEntryCategory = getSpecificSetter(addedEntry, setAddedEntry, 'categoryCode') as ((value: typeof addedEntry.categoryCode) => void);
    const setAddedEntryTime = (time: number) => setAddedEntry(prevValue => ({ ...prevValue, time: Math.max(time, 1) }));
    const setAddedEntryDescription = getSpecificSetter(addedEntry, setAddedEntry, 'description') as ((value: typeof addedEntry.description) => void);
    const clearAddedEntry = () => {
        setAddedEntry({
            projectCode: projectList?.at(0)?.code ?? '',
            categoryCode: '',
            time: 1,
            description: ''
        });
    }

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

    const appendEntry = () => {
        ApiConnector.addReportEntry(lastDateState.state.lastDate, addedEntry)
            .then(() => {
                clearAddedEntry();
                refreshDailyReport();
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
                    <tr>
                        <td className="shrinked">
                            <Form.Select value={addedEntry.projectCode} onChange={evt => setAddedEntryProject(evt.target.value)}>
                                {projectList?.map(project => (
                                    <option key={project.code} value={project.code}>{project.name} ({project.code})</option>
                                ))}
                            </Form.Select>
                        </td>
                        <td className="shrinked">
                            <Form.Select value={addedEntry.categoryCode} onChange={evt => setAddedEntryCategory(evt.target.value)}>
                                {projectList?.filter(e => e.code === addedEntry.projectCode)[0]?.categories.map(category => (
                                    <option key={category.code} value={category.code}>{category.code}</option>
                                ))}
                            </Form.Select>
                        </td>
                        <td className="shrinked">
                            <Form.Control type="number" value={addedEntry.time} min={1} onChange={evt => setAddedEntryTime(Number(evt.target.value))} />
                        </td>
                        <td>
                            <Form.Control as="textarea" value={addedEntry.description} onChange={evt => setAddedEntryDescription(evt.target.value)} />
                        </td>
                        <td className="shrinked">
                            <ButtonToolbar className="flex-md-nowrap">
                                <Button className="me-2" onClick={() => appendEntry()}>Zatwierdź</Button>
                                <Button className="me-2" onClick={clearAddedEntry}>Czyść</Button>
                            </ButtonToolbar>
                        </td>
                    </tr>
                </tfoot>
            </Table>
            {dailyReport !== null && projectList !== null ? (<>
                <h2>Podsumowanie dzienne</h2>
                <DailySummary entries={dailyReport.entries} projects={projectList} />
            </>) : <></>}
        </>
    );
}

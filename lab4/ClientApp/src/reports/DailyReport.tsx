import React, {useContext, useEffect, useState} from 'react';
import {LastDateContext} from "../App";
import DailyReportModel from "../models/DailyReport";
import fetchData from "../fetchData";
import {Button, ButtonToolbar, Form, Table} from "react-bootstrap";
import Project from "../models/Project";
import ReportEntryCreationRequest from "../models/ReportEntryCreationRequest";
import DailySummary from "../reports/DailySummary";
import '../custom.css';
import ReportEntryUpdateRequest from "../models/ReportEntryUpdateRequest";
import getSpecificSetter from "../getSpecificSetter";
import ReportEntry from "../models/ReportEntry";

export default function DailyReport() {
    const lastDateState = useContext(LastDateContext);

    const [dailyReport, setDailyReport] = useState<DailyReportModel | null>(null);
    const setDailyReportEntry = (id: number | null, updatedEntry: ReportEntry) => {
        if (id === null)
            return;
        setDailyReport((prevState) => {
            if (prevState === null || !(id in prevState.entries))
                return null;
            return { ...prevState, entries: { ...prevState.entries, [id]: updatedEntry } };
        });
    };
    const refreshDailyReport = () => {
        fetchData(`/api/reports/${lastDateState.state.lastDate}`)
            .then(data => setDailyReport(data));
    }
    useEffect(refreshDailyReport, [lastDateState.state.lastDate]);

    const [projectList, setProjectList] = useState<Project[] | null>(null);
    useEffect(() => {
        fetchData('/api/projects')
            .then(data => {
                setProjectList(data);
                if (data.length > 0)
                    setAddedEntryProject(data[0].code);
            });
    }, []);

    const [addedEntry, setAddedEntry] = useState<ReportEntryCreationRequest>({
        projectCode: '',
        categoryCode: '',
        time: 0,
        description: ''
    });
    const setAddedEntryProject = (projectCode: string) => setAddedEntry(prevState => ({...prevState, projectCode: projectCode, categoryCode: '' }));
    const setAddedEntryCategory = getSpecificSetter(addedEntry, setAddedEntry, 'categoryCode') as ((value: typeof addedEntry.categoryCode) => void);
    const setAddedEntryTime = getSpecificSetter(addedEntry, setAddedEntry, 'time') as ((value: typeof addedEntry.time) => void);
    const setAddedEntryDescription = getSpecificSetter(addedEntry, setAddedEntry, 'description') as ((value: typeof addedEntry.description) => void);
    const clearAddedEntry = () => {
        setAddedEntry({
            projectCode: projectList?.at(0)?.code ?? '',
            categoryCode: '',
            time: 0,
            description: ''
        });
    }

    const [modifiedEntryId, setModifiedEntryId] = useState<number | null>(null);
    const [modifiedEntry, setModifiedEntry] = useState<ReportEntry>({
        date: '',
        projectCode: '',
        categoryCode: '',
        time: 0,
        description: '',
        timestamp: ''
    });
    const setModifiedEntryCategory = getSpecificSetter(modifiedEntry, setModifiedEntry, 'categoryCode') as ((value: typeof modifiedEntry.categoryCode) => void);
    const setModifiedEntryTime = getSpecificSetter(modifiedEntry, setModifiedEntry, 'time') as ((value: typeof modifiedEntry.time) => void);
    const setModifiedEntryDescription = getSpecificSetter(modifiedEntry, setModifiedEntry, 'description') as ((value: typeof modifiedEntry.description) => void);

    useEffect(() => {
        if (modifiedEntryId === null || dailyReport === null)
            return;
        if (!(modifiedEntryId in dailyReport.entries))
            return;
        setModifiedEntry({ ...dailyReport.entries[modifiedEntryId] });
    }, [modifiedEntryId, dailyReport]);

    const updateEntry = () => {
        if (!modifiedEntry)
            return;
        let updateRequest: ReportEntryUpdateRequest = {
            categoryCode: modifiedEntry.categoryCode,
            time: modifiedEntry.time,
            description: modifiedEntry.description,
            timestamp: modifiedEntry.timestamp
        };
        fetchData(`/api/reportentries/${modifiedEntryId}`, 'PATCH', updateRequest)
            .then(() => {
                setModifiedEntryId(null);
                refreshDailyReport();
            })
            .catch(error => alert(error));
    };

    const appendEntry = () => {
        fetchData(`/api/reports/${lastDateState.state.lastDate}/entries`, 'POST', addedEntry)
            .then(() => {
                clearAddedEntry();
                refreshDailyReport();
            })
            .catch(error => alert(error));
    };

    const removeEntry = (id: number) => {
        fetchData(`/api/reportentries/${id}`, 'DELETE')
            .then(() => {
                refreshDailyReport();
            })
            .catch(error => alert(error));
    }

    const getEditView = (id: number, entry: ReportEntry) => (<tr key={id}>
        <td className="shrinked">{entry.projectCode}</td>
        <td className="shrinked">
            <Form.Select value={modifiedEntry.categoryCode} onChange={evt => setModifiedEntryCategory(evt.target.value)}>
                {projectList?.filter(e => e.code === modifiedEntry.projectCode)[0]?.categories.map(category => (
                    <option key={category.code} value={category.code}>{category.code}</option>
                ))}
            </Form.Select>
        </td>
        <td className="shrinked">
            <Form.Control type="number" value={modifiedEntry.time} onChange={evt => setModifiedEntryTime(Number(evt.target.value))} />
        </td>
        <td>
            <Form.Control as="textarea" value={modifiedEntry.description} onChange={evt => setModifiedEntryDescription(evt.target.value)} />
        </td>
        <td className="shrinked">
            <ButtonToolbar className="flex-md-nowrap">
                <Button className="me-2" onClick={() => updateEntry()}>Zatwierdź</Button>
                <Button className="me-2" onClick={() => setModifiedEntryId(null)}>Anuluj</Button>
            </ButtonToolbar>
        </td>
    </tr>);

    const getDisplayView = (id: number, entry: ReportEntry) => (<tr key={id}>
        <td className="shrinked">{entry.projectCode}</td>
        <td className="shrinked">{entry.categoryCode}</td>
        <td className="shrinked">{entry.time}</td>
        <td>{entry.description}</td>
        <td className="shrinked">
            <ButtonToolbar className="flex-md-nowrap">
                <Button className="me-2" disabled={dailyReport?.frozen} onClick={() => setModifiedEntryId(id)}>Edytuj</Button>
                <Button className="me-2" disabled={dailyReport?.frozen} onClick={() => removeEntry(id)}>Skasuj</Button>
            </ButtonToolbar>
        </td>
    </tr>);

    return (
        <>
            <h1>Raport czasu pracy na dzień {lastDateState.state.lastDate}</h1>
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
                    {Object.entries(dailyReport?.entries ??  {}).map(e => ({ key: Number(e[0]), value: e[1] }))
                        .map(({key, value}) => key === modifiedEntryId ? getEditView(key, value) : getDisplayView(key, value))}
                </tbody>
                <tfoot style={{ verticalAlign: 'top' }}>
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
                            <Form.Control type="number" value={addedEntry.time} onChange={evt => setAddedEntryTime(Number(evt.target.value))} />
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

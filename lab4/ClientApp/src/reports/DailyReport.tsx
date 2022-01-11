import React, {useContext, useEffect, useState} from 'react';
import {LastDateContext} from "../App";
import DailyReportModel from "../models/DailyReport";
import fetchData from "../fetchData";
import {Button, ButtonToolbar, Form, Table} from "react-bootstrap";
import Project from "../models/Project";
import ReportEntryCreationRequest from "../models/ReportEntryCreationRequest";
import DailySummary from "../reports/DailySummary";
import '../custom.css';

export default function DailyReport() {
    const lastDateState = useContext(LastDateContext);
    const [dailyReport, setDailyReport] = useState<DailyReportModel | null>(null);
    const [projectList, setProjectList] = useState<Project[] | null>(null);
    const [addedEntry, setAddedEntry] = useState<ReportEntryCreationRequest>({
        projectCode: '',
        categoryCode: '',
        time: 0,
        description: ''
    });
    useEffect(() => {
        fetchData(`/api/reports/${lastDateState.lastDate}`)
            .then(data => setDailyReport(data));
    }, [lastDateState.lastDate]);
    useEffect(() => {
        fetchData('/api/projects')
            .then(data => {
                setProjectList(data);
                if (data.length > 0)
                    setAddedEntryProject(data[0].code);
            });
    }, []);

    const setAddedEntryProject = (projectCode: string) => {
        setAddedEntry(prevState => ({...prevState, projectCode: projectCode, categoryCode: '' }))
    };

    const setAddedEntryCategory = (categoryCode: string) => {
        setAddedEntry(prevState => ({...prevState, categoryCode: categoryCode }))
    };

    const setAddedEntryTime = (time: number) => {
        setAddedEntry(prevState => ({...prevState, time: time }))
    };

    const setAddedEntryDescription = (description: string) => {
        setAddedEntry(prevState => ({...prevState, description: description }))
    };

    const clearAddedEntry = () => {
        setAddedEntry({
            projectCode: projectList?.at(0)?.code ?? '',
            categoryCode: '',
            time: 0,
            description: ''
        });
    }

    return (
        <>
            <h1>Raport czasu pracy na dzień {lastDateState.lastDate}</h1>
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
                    {dailyReport?.entries.map(entry => (<tr key={entry.id}>
                        <td className="shrinked">{entry.projectCode}</td>
                        <td className="shrinked">{entry.categoryCode}</td>
                        <td className="shrinked">{entry.time}</td>
                        <td>{entry.description}</td>
                        <td className="shrinked">
                            <ButtonToolbar className="flex-md-nowrap">
                                <Button className="me-2" disabled={dailyReport?.frozen}>Edytuj</Button>
                                <Button className="me-2" disabled={dailyReport?.frozen}>Skasuj</Button>
                            </ButtonToolbar>
                        </td>
                    </tr>))}
                </tbody>
                <tfoot style={{ verticalAlign: 'top' }}>
                    <tr>
                        <td colSpan={5}>Dodaj wpis...</td>
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
                                <Button className="me-2">Zatwierdź</Button>
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

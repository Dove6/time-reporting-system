import {Button, ButtonToolbar, Form} from "react-bootstrap";
import React, { useEffect, useState } from "react";
import '../custom.css';
import ReportEntryCreationRequest from "../models/ReportEntryCreationRequest";
import getSpecificSetter from "../getSpecificSetter";
import ProjectModel from "../models/ProjectModel";

type ReportEntryCreatorProps = {
    projects: ProjectModel[] | null;
    onConfirmClick: (created: ReportEntryCreationRequest) => Promise<void>;
};

export default function ReportEntryCreator(props: ReportEntryCreatorProps) {
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
            projectCode: props.projects?.at(0)?.code ?? '',
            categoryCode: '',
            time: 1,
            description: ''
        });
    }

    useEffect(() => {
        if (props.projects && props.projects.length > 0)
            setAddedEntryProject(props.projects[0].code);
        else
            setAddedEntryProject('');
    }, [props.projects]);

    const addEntry = () => {
        props.onConfirmClick(addedEntry)
            .then(clearAddedEntry);
    }

    return (<tr>
        <td className="shrinked">
            <Form.Select value={addedEntry.projectCode} onChange={evt => setAddedEntryProject(evt.target.value)}>
                {props.projects?.map(project => (
                    <option key={project.code} value={project.code}>{project.name} ({project.code})</option>
                ))}
            </Form.Select>
        </td>
        <td className="shrinked">
            <Form.Select value={addedEntry.categoryCode} onChange={evt => setAddedEntryCategory(evt.target.value)}>
                {props.projects?.filter(e => e.code === addedEntry.projectCode)[0]?.categories.map(category => (
                    <option key={category.code} value={category.code}>{category.code}</option>
                ))}
            </Form.Select>
        </td>
        <td className="shrinked">
            <Form.Control type="number" value={addedEntry.time} min={1}
                onChange={evt => setAddedEntryTime(Number(evt.target.value))} />
        </td>
        <td>
            <Form.Control as="textarea" value={addedEntry.description}
                onChange={evt => setAddedEntryDescription(evt.target.value)} />
        </td>
        <td className="shrinked">
            <ButtonToolbar className="flex-md-nowrap">
                <Button className="me-2" onClick={addEntry}>Zatwierdź</Button>
                <Button className="me-2" onClick={clearAddedEntry}>Czyść</Button>
            </ButtonToolbar>
        </td>
    </tr>);
}

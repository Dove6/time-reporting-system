import {Button, ButtonToolbar, Form} from "react-bootstrap";
import React, {MouseEventHandler, useState} from "react";
import ReportEntryModel from '../models/ReportEntryModel';
import ReportEntryUpdateRequest from "../models/ReportEntryUpdateRequest";
import getSpecificSetter from "../getSpecificSetter";
import CategoryModel from "../models/CategoryModel";

type EditableReportEntryProps = {
    original: ReportEntryModel;
    categories: CategoryModel[];
    onConfirmClick: (modified: ReportEntryUpdateRequest) => void;
    onCancelClick: MouseEventHandler;
};

export default function EditableReportEntry(props: EditableReportEntryProps) {
    const [modifiedEntry, setModifiedEntry] = useState<ReportEntryModel>({ ...props.original });
    const setModifiedEntryCategory = getSpecificSetter(modifiedEntry, setModifiedEntry, 'categoryCode') as ((value: typeof modifiedEntry.categoryCode) => void);
    const setModifiedEntryTime = (time: number) => setModifiedEntry(prevValue => ({ ...prevValue, time: Math.max(time, 1) }));
    const setModifiedEntryDescription = getSpecificSetter(modifiedEntry, setModifiedEntry, 'description') as ((value: typeof modifiedEntry.description) => void);

    const updateEntry = () => props.onConfirmClick({
        categoryCode: modifiedEntry.categoryCode,
        time: modifiedEntry.time,
        description: modifiedEntry.description
    });

    return (<tr>
        <td className="shrinked">{props.original.projectCode}</td>
        <td className="shrinked">
            <Form.Select value={modifiedEntry.categoryCode} onChange={evt => setModifiedEntryCategory(evt.target.value)}>
                {props.categories.map(category => (
                    <option key={category.code} value={category.code}>{category.code}</option>
                ))}
            </Form.Select>
        </td>
        <td className="shrinked">
            <Form.Control type="number" value={modifiedEntry.time} min={1}
                onChange={evt => setModifiedEntryTime(Number(evt.target.value))} />
        </td>
        <td>
            <Form.Control as="textarea" value={modifiedEntry.description}
                onChange={evt => setModifiedEntryDescription(evt.target.value)} />
        </td>
        <td className="shrinked">
            <ButtonToolbar className="flex-md-nowrap">
                <Button className="me-2" onClick={updateEntry}>Zatwierdź</Button>
                <Button className="me-2" onClick={props.onCancelClick}>Anuluj</Button>
            </ButtonToolbar>
        </td>
    </tr>);
}

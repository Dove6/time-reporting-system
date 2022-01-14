import {Button, ButtonToolbar} from "react-bootstrap";
import React, {MouseEventHandler} from "react";
import ReportEntryModel from '../models/ReportEntryModel';

type ReportEntryProps = {
    content: ReportEntryModel;
    disabled: boolean;
    onModifyClick: MouseEventHandler;
    onRemoveClick: MouseEventHandler;
};

export default function ReportEntry(props: ReportEntryProps) {
    return (<tr>
        <td className="shrinked">{props.content.projectCode}</td>
        <td className="shrinked">{props.content.categoryCode}</td>
        <td className="shrinked">{props.content.time}</td>
        <td>{props.content.description}</td>
        <td className="shrinked">
            <ButtonToolbar className="flex-md-nowrap">
                <Button className="me-2" disabled={props.disabled} onClick={props.onModifyClick}>Edytuj</Button>
                <Button className="me-2" disabled={props.disabled} onClick={props.onRemoveClick}>Skasuj</Button>
            </ButtonToolbar>
        </td>
    </tr>);
}

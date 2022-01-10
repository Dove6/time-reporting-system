import AcceptedTime from "./AcceptedTime";
import ManagedProject from "./ManagedProject";

type ProjectDetails = ManagedProject & {
    acceptedTime: AcceptedTime[];
}

export default ProjectDetails;

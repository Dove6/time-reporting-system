import ProjectCreationRequest from "./ProjectCreationRequest";

type ProjectUpdateRequest = ProjectCreationRequest & {
    timestamp: string;
}

export default ProjectUpdateRequest;

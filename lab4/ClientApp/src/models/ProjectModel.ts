import CategoryModel from './CategoryModel';

type ProjectModel = {
    code: string;
    name: string;
    categories: CategoryModel[];
}

export default ProjectModel;

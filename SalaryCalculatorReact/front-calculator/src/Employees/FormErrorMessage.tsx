import React from "react";

export interface IResponse {
    title: string
    errors: any
}

export const FormErrorMessage: React.FC<IResponse> = ({ title, errors }) =>(
    <div>
        {title}
        {Object.keys(errors).map((key) => (<p key={key}>{errors[key]}</p>))}
    </div>
)

    

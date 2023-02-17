import { Fragment } from "react";

const Billing = () => {
    return (
        <Fragment>
            <p className="text-gray-700 text-3xl mb-8 mt-2 font-bold">
                Billing
            </p>

            <div className="grid lg:grid-cols-3 gap-5 mb-16">
                <div className="rounded bg-gray-300 h-40 shadow-sm"></div>
                <div className="rounded bg-gray-300 h-40 shadow-sm"></div>
                <div className="rounded bg-gray-300 h-40 shadow-sm"></div>
            </div>
            <div className="grid col-1 bg-gray-300 h-96 shadow-sm"></div>
        </Fragment>
    );
};

export default Billing;

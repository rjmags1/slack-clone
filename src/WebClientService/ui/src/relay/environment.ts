import {
    Store,
    RecordSource,
    Environment,
    Network,
    Observable,
} from 'relay-runtime'
import type { FetchFunction, IEnvironment } from 'relay-runtime'

const fetchFn: FetchFunction = (params, variables) => {
    const response = fetch('/remote/graphql', {
        method: 'POST',
        headers: [
            ['Content-Type', 'application/json'],
            ['X-CSRF', '1'],
        ],
        body: JSON.stringify({
            query: params.text,
            variables,
        }),
    })

    return Observable.from(response.then((data) => data.json()))
}

export function createEnvironment(): IEnvironment {
    const network = Network.create(fetchFn)
    const store = new Store(new RecordSource())
    return new Environment({ store, network })
}

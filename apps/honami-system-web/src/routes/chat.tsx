import {createFileRoute, Outlet} from '@tanstack/react-router'

export const Route = createFileRoute('/chat')({
    component: RouteComponent,
})

function RouteComponent() {
    return (
        <div className="flex flex-1 p-0 m-0">
            <aside>
                <h2>Chat List</h2>
                <ul>
                    <li>Chat 1</li>
                    <li>Chat 2</li>
                    <li>Chat 3</li>
                </ul>
            </aside>
            <div className="w-full bg-green-400">
                <Outlet/>
            </div>
        </div>
    )
}

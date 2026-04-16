import {createFileRoute} from '@tanstack/react-router'

export const Route = createFileRoute('/chat/$chatID')({
    component: RouteComponent,
})

function RouteComponent() {
    return (
        <div>
            Chat Content
        </div>
    )
}

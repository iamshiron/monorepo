import { useEffect, useState } from 'react';
import { Button } from '@shiron/ui/components/ui/button';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@shiron/ui/components/ui/dialog';
import { Input } from '@shiron/ui/components/ui/input';
import { Label } from '@shiron/ui/components/ui/label';
import { Textarea } from '@shiron/ui/components/ui/textarea';
import type { CollectionEntry } from '@/types';

export function EditModal({
    entry,
    isOpen,
    onClose,
    onSave,
}: {
    entry: CollectionEntry | null;
    isOpen: boolean;
    onClose: () => void;
    onSave: (
        id: string,
        data: { notes?: string; keyCount?: number },
    ) => Promise<void>;
}) {
    const [notes, setNotes] = useState('');
    const [keyCount, setKeyCount] = useState<number | ''>('');
    const [isSaving, setIsSaving] = useState(false);

    useEffect(() => {
        if (entry) {
            setNotes(entry.notes || '');
            setKeyCount(entry.character.keyCount ?? '');
        }
    }, [entry]);

    const handleSave = async () => {
        if (!entry) return;
        setIsSaving(true);
        try {
            await onSave(entry.id, {
                notes: notes || undefined,
                keyCount: keyCount === '' ? undefined : keyCount,
            });
            onClose();
        } finally {
            setIsSaving(false);
        }
    };

    return (
        <Dialog
            open={isOpen}
            onOpenChange={(open) => {
                if (!open) onClose();
            }}
        >
            <DialogContent className="sm:max-w-md">
                <DialogHeader>
                    <DialogTitle>Edit Character</DialogTitle>
                    <DialogDescription>
                        {entry?.character.name}
                    </DialogDescription>
                </DialogHeader>

                <div className="space-y-4">
                    <div>
                        <Label htmlFor="edit-key-count" className="mb-1">
                            Key Count
                        </Label>
                        <Input
                            id="edit-key-count"
                            type="number"
                            value={keyCount}
                            onChange={(e) =>
                                setKeyCount(
                                    e.target.value === ''
                                        ? ''
                                        : Number(e.target.value),
                                )
                            }
                            min={0}
                            placeholder="0"
                            className="h-9"
                        />
                        <p className="text-xs text-muted-foreground/70 mt-1">
                            Key type is automatically determined: Bronze (1-2),
                            Silver (3-5), Gold (6-9), Chaos (10+)
                        </p>
                    </div>

                    <div>
                        <Label htmlFor="edit-notes" className="mb-1">
                            Notes
                        </Label>
                        <Textarea
                            id="edit-notes"
                            value={notes}
                            onChange={(e) => setNotes(e.target.value)}
                            placeholder="Add notes about this character..."
                            rows={4}
                            className="resize-none"
                        />
                    </div>
                </div>

                <DialogFooter>
                    <Button variant="outline" onClick={onClose}>
                        Cancel
                    </Button>
                    <Button onClick={handleSave} disabled={isSaving}>
                        {isSaving ? 'Saving...' : 'Save'}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
}

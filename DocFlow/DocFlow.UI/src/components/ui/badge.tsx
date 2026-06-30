import * as React from 'react'
import { cn } from '@/lib/utils'

function Badge({ className, variant = 'default', ...props }: React.ComponentProps<'span'> & { variant?: 'default' | 'secondary' | 'outline' }) {
  return (
    <span
      className={cn(
        'inline-flex items-center rounded-md border px-2.5 py-0.5 text-xs font-semibold transition-colors',
        variant === 'default' && 'border-transparent bg-foreground text-background',
        variant === 'secondary' && 'border-transparent bg-muted text-muted-foreground',
        variant === 'outline' && 'text-foreground',
        className,
      )}
      {...props}
    />
  )
}

export { Badge }
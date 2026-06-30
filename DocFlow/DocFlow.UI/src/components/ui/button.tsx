import * as React from 'react'
import { cn } from '@/lib/utils'

function Button({ className, variant = 'default', size = 'default', ...props }: React.ButtonHTMLAttributes<HTMLButtonElement> & { variant?: 'default' | 'secondary' | 'outline'; size?: 'default' | 'sm' | 'lg' }) {
  return (
    <button
      className={cn(
        'inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:pointer-events-none disabled:opacity-50',
        variant === 'default' && 'bg-foreground text-background shadow hover:opacity-90',
        variant === 'secondary' && 'bg-muted text-foreground hover:bg-muted/80',
        variant === 'outline' && 'border border-border bg-transparent hover:bg-muted',
        size === 'default' && 'h-9 px-4 py-2',
        size === 'sm' && 'h-8 rounded-md px-3 text-xs',
        size === 'lg' && 'h-10 rounded-md px-8',
        className,
      )}
      {...props}
    />
  )
}

export { Button }
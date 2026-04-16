export function useCurrency() {
  function format(amount: number) {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'TRY',
      maximumFractionDigits: 2
    }).format(amount)
  }

  return { format }
}

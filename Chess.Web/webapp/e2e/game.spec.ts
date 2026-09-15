import { expect, test } from '@playwright/test';

test('starts a game through the webapp and API', async ({ page }) => {
  await page.goto('/');

  await expect(page.getByRole('button', { name: 'Start game' })).toBeVisible();
  await page.getByRole('button', { name: 'Start game' }).click();

  const board = page.getByLabel('Chess board');
  await expect(board).toBeVisible({ timeout: 45_000 });
  await expect(board.locator('button')).toHaveCount(64);
});

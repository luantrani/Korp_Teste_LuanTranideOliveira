import 'zone.js/testing';
import { getTestBed } from '@angular/core/testing';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';

describe('setup', () => {
  beforeAll(() => {
    getTestBed().initTestEnvironment(BrowserDynamicTestingModule, platformBrowserDynamicTesting());
  });

  it('should setup test environment', () => {
    expect(true).toBeTrue();
  });
});

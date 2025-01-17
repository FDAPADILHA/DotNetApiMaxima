import { ComponentFixture, TestBed } from '@angular/core/testing';

import { departamentoComponent } from './departamento.component';

describe('DepartamentoComponent', () => {
  let component: departamentoComponent;
  let fixture: ComponentFixture<departamentoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ departamentoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(departamentoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

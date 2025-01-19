import { Component } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {
  isSidebarOpen: boolean = false;
  menuOpen: boolean = false;

  toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }

  cadastroDepartamento() {
    console.log('Cadastro de Departamento');
  }
}

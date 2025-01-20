import { Component } from '@angular/core';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  isVisible: boolean = false;

  toggleNavbar() {
    this.isVisible = !this.isVisible;
  }

  logout() {
    console.log("Deslogando do sistema...");
  }
}
